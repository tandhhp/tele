import { apiCreatePlasmaCheckIn, apiDeletePlasmaCheckIn, apiPlasmaCheckInList, apiPlasmaUser, apiUpdatePlasmaCheckIn } from "@/services/plasmaCheckIn";
import { DrawerForm, PageContainer, ProFormDatePicker, ProFormSelect, ProFormText, ProTable } from "@ant-design/pro-components"
import { Button, message, Popconfirm, Tooltip } from "antd";
import { useForm } from "antd/es/form/Form";
import { useEffect, useRef, useState } from "react";
import type { ActionType } from "@ant-design/pro-components";
import { DeleteOutlined, EditOutlined, ManOutlined, PlusOutlined, WomanOutlined } from "@ant-design/icons";
import dayjs, { Dayjs } from "dayjs";
import { Branch, BRANCH_OPTIONS } from "@/utils/constants";

interface PlasmaUser {
    value: string;
    label: string;
    gender: number;
    phoneNumber: string;
    totalDays: number;
    supporterName: string;
    branch: string;
}

const PlasmaCheckInPage: React.FC = () => {
    const [form] = useForm();
    const [open, setOpen] = useState(false);
    const actionRef = useRef<ActionType>();
    const [editingRecord, setEditingRecord] = useState<any>(null);
    const [plasmaUser, setPlasmaUser] = useState<PlasmaUser[]>([]);

    useEffect(() => {
        apiPlasmaUser().then((res) => {
            console.log('Plasma:', res);
            setPlasmaUser(res);
        });
    }, []);

    const handleSubmit = async (values: any) => {
        const payload = {
            ...values,
            date: values.date ? dayjs(values.date).format('YYYY-MM-DD') : undefined,
        };
        try {
            if (editingRecord) {
                console.log('Editing record:', editingRecord.id);
                await apiUpdatePlasmaCheckIn({ id: editingRecord.id, ...payload });
                message.success('Cập nhật thành công!');
            } else {
                await apiCreatePlasmaCheckIn(payload);
                message.success('Tạo mới thành công!');
            }
            actionRef.current?.reload();
            setOpen(false);
            setEditingRecord(null);
        } catch (error) {
            console.error('Error creating/updating category:', error);
            message.error(editingRecord ? 'Cập nhật thất bại!' : 'Tạo mới thất bại!');
        }
    };

    const handleDelete = async (record: any) => {
        try {
            await apiDeletePlasmaCheckIn(record.id);
            message.success('Xoá thành công!');
            actionRef.current?.reload();
        } catch (error) {
            console.error('Error deleting category:', error);
            message.error('Xoá thất bại!');
        }
    };
    return (
        <PageContainer>
            <ProTable actionRef={actionRef} request={apiPlasmaCheckInList} search={{ layout: 'vertical' }}
                scroll={{ x: 'max-content' }}
                headerTitle={
                    <Button
                        type="primary"
                        icon={<PlusOutlined />}
                        onClick={() => {
                            form.resetFields(); // 👈 Reset form trước khi mở
                            setEditingRecord(null);
                            setOpen(true);
                        }}
                    >
                        Tạo mới
                    </Button>
                }
                columns={[
                    { title: '#', valueType: 'indexBorder', width: 30 },
                    { title: 'Họ và tên', dataIndex: 'fullName',
                        render: (text, record) => {
                            if (record.gender) {
                                return <><ManOutlined className="text-blue-500 mr-1" />{text}</>
                            }
                            return <span><WomanOutlined className="text-red-500 mr-1" />{text}</span>;
                        }
                     },
                    { title: 'SĐT', dataIndex: 'phoneNumber' },
                    { title: 'Số ngày', dataIndex: 'totalDays', search: false },
                    { title: 'Chuyên viên hỗ trợ', dataIndex: 'supporterName', search: false },
                    {
                        title: 'Chi nhánh', dataIndex: 'branch', search: false, 
                        fieldProps: {
                            options: BRANCH_OPTIONS
                        }
                    },
                    {
                        title: 'Ngày Check-In', dataIndex: 'date', valueType: 'date', search: false,
                        render: (_, record) => record.date ? dayjs(record.date).format('DD-MM-YYYY') : '',
                        width: 110
                    },
                    {
                        title: 'Giờ Check In', dataIndex: 'time', search: false,
                        valueEnum: {
                            0: { text: '08:15' },
                            1: { text: '09:15' },
                            2: { text: '10:15' },
                            3: { text: '11:15' },
                            4: { text: '13:30' },
                            5: { text: '14:30' },
                            6: { text: '15:30' },
                            7: { text: '16:30' },
                        },
                        minWidth: 90
                    },
                    {
                        title: 'Loại Plasma', dataIndex: 'plasmaType', search: false, valueEnum: {
                            0: { text: 'DDS'},
                            1: { text: 'PLM'},
                            2: { text: 'FCB'},
                        }
                    },
                    {
                        title: 'Ngày tạo', dataIndex: 'createdDate', valueType: 'fromNow', search: false
                    },
                    {
                        title: 'Ngày cập nhật', dataIndex: 'modifiedDate', valueType: 'date', search: false, align: 'center',
                        render: (_, record) => record.modifiedDate ? dayjs(record.modifiedDate).format('DD-MM-YYYY') : ''
                    },
                    {
                        title: 'Trạng thái',
                        dataIndex: 'status',
                        valueEnum: {
                            0: { text: 'Chưa Check In', status: 'Default' },
                            1: { text: 'Đã Check In', status: 'Success' },
                            2: { text: 'Đã Huỷ', status: 'Error' }
                        }
                    },
                    {
                        title: 'Tác vụ',
                        valueType: 'option',
                        width: 60,
                        fixed: 'right',
                        render: (text, record) => [
                            <Tooltip key={`edit-${record.id}`} title="Chỉnh sửa">
                                <Button
                                    icon={<EditOutlined />}
                                    size="small"
                                    onClick={() => {
                                        setEditingRecord(record);
                                        form.setFieldsValue({
                                            plasmaUserId: record.plasmaUserId,
                                            fullName: record.fullName,
                                            gender: record.gender === true ? 'Nam' : 'Nữ',
                                            Email: record.email,
                                            phoneNumber: record.phoneNumber,
                                            totalDays: record.totalDays,
                                            supporterName: record.supporterName,
                                            branch: record.branch === Branch.South ? 'Miền Nam' : 'Miền Bắc',
                                            date: record.date ? dayjs(record.date) : null,
                                            time: record.time,
                                        });
                                        setOpen(true);
                                    }}
                                />
                            </Tooltip>,
                            <Popconfirm
                                key={`delete-confirm-${record.id}`}
                                title="Bạn có chắc chắn muốn xoá?"
                                description={`Tên: ${record.fullName}`}
                                okText="Xoá"
                                onConfirm={() => handleDelete(record)}
                                cancelText="Huỷ"
                            >
                                <Tooltip title="Xoá">
                                    <Button
                                        size="small"
                                        danger
                                        icon={<DeleteOutlined />}
                                    />
                                </Tooltip>
                            </Popconfirm>,
                        ]
                    }

                ]}
            />
            <DrawerForm
                form={form}
                open={open}
                grid={true}
                drawerProps={{ width: 600 }}
                rowProps={{ gutter: 16 }}
                onOpenChange={(visible) => {
                    if (!visible) {
                        setEditingRecord(null);
                    }
                    setOpen(visible);
                }}
                onFinish={handleSubmit}
                onValuesChange={(changedValues) => {
                    if (changedValues.plasmaUserId) {
                        const selected = plasmaUser.find(p => p.value === changedValues.plasmaUserId);
                        if (selected) {
                            form.setFieldsValue({
                                gender: selected.gender,
                                phoneNumber: selected.phoneNumber,
                                totalDays: selected.totalDays,
                                supporterName: selected.supporterName,
                                branch: selected.branch,
                            });
                        }
                    }
                }}
                initialValues={{
                    Name: editingRecord?.name,
                    Description: editingRecord?.description,
                    Active: editingRecord?.active,
                }}
            >
                <ProFormSelect
                    name="plasmaUserId"
                    label="Khách Plasma"
                    rules={[{ required: true, message: 'Vui lòng chọn khách Plasma' }]}
                    colProps={{ span: 12 }}
                    showSearch
                    options={plasmaUser}
                />
                <ProFormText fieldProps={{ readOnly: true }} name="gender" label="Giới tính" colProps={{ span: 12 }} />
                <ProFormText name="phoneNumber" label="SĐT" width="lg" colProps={{ span: 12 }} fieldProps={{ readOnly: true }} />
                <ProFormText name="totalDays" label="Tổng số ngày" width="lg" colProps={{ span: 12 }} fieldProps={{ readOnly: true }} />
                <ProFormText name="supporterName" label="Chuyên viên hỗ trợ" width="lg" colProps={{ span: 12 }} fieldProps={{ readOnly: true }} />
                <ProFormText name="branch" label="Chi nhánh" colProps={{ span: 12 }} fieldProps={{ readOnly: true }} />
                <ProFormDatePicker
                    name="date"
                    label="Ngày"
                    width="lg"
                    rules={[{ required: true, message: 'Vui lòng nhập ngày' }]}
                    colProps={{ span: 12 }}
                    fieldProps={{
                        disabledDate: (current: Dayjs) => current && current < dayjs().startOf('day'),
                        format: {
                            type: 'mask',
                            format: 'DD-MM-YYYY'
                        }
                    }}
                />
                <ProFormSelect
                    name="time"
                    label="Khung giờ"
                    rules={[{ required: true, message: 'Vui lòng chọn khung giờ' }]}
                    colProps={{ span: 12 }}
                    options={[
                        { label: '08:15', value: '08:15:00' },
                        { label: '09:15', value: '09:15:00' },
                        { label: '10:15', value: '10:15:00' },
                        { label: '11:15', value: '11:15:00' },
                        { label: '13:30', value: '13:30:00' },
                        { label: '14:30', value: '14:30:00' },
                        { label: '15:30', value: '15:30:00' },
                        { label: '16:30', value: '16:30:00' },
                    ]}
                />
                <ProFormSelect
                    rules={[{ required: true, message: 'Vui lòng chọn loại Plasma' }]}
                    options={[
                        { label: 'DDS', value: 0 },
                        { label: 'PLM', value: 1 },
                        { label: 'FCB', value: 2 },
                    ]} name="plasmaType" label="Loại Plasma" colProps={{ span: 12 }} />
            </DrawerForm>
        </PageContainer>
    );
};

export default PlasmaCheckInPage;