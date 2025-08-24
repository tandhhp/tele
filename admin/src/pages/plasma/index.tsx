import { apiCreatePlasma, apiDeletePlasma, apiPlasmaList, apiTechicianOptions, apiUpdatePlasma } from "@/services/plasma";
import { DrawerForm, PageContainer, ProFormDatePicker, ProFormSelect, ProFormText, ProTable } from "@ant-design/pro-components"
import { Button, message, Popconfirm, Tooltip } from "antd";
import { useForm } from "antd/es/form/Form";
import { useEffect, useRef, useState } from "react";
import type { ActionType } from "@ant-design/pro-components";
import { DeleteOutlined, EditOutlined, ManOutlined, PlusOutlined, WomanOutlined } from "@ant-design/icons";
import dayjs from "dayjs";
import { BRANCH_OPTIONS } from "@/utils/constants";

interface TechnicianOption {
    value: string;
    label: string;
    branch: number;
}

const PlasmaPage: React.FC = () => {
    const [techicianOptions, setTechicianOptions] = useState<TechnicianOption[]>([]);
    const [form] = useForm();
    const [open, setOpen] = useState(false);
    const actionRef = useRef<ActionType>();
    const [editingRecord, setEditingRecord] = useState<any>(null);
    useEffect(() => {
        apiTechicianOptions().then((res) => {
            console.log('Technician options:', res);
            setTechicianOptions(res);
        });
    }, []);
    const handleSubmit = async (values: any) => {
        const payload = {
            ...values,
            dateOfBirth: values.dateOfBirth ? dayjs(values.dateOfBirth).format('YYYY-MM-DD') : undefined,
        };
        try {
            console.log('Form values:', editingRecord);
            if (editingRecord) {
                await apiUpdatePlasma({ id: editingRecord.id, ...payload });
                message.success('Cập nhật thành công!');
            } else {
                await apiCreatePlasma(payload);
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
            await apiDeletePlasma(record.id);
            message.success('Xoá thành công!');
            actionRef.current?.reload();
        } catch (error) {
            console.error('Error deleting category:', error);
            message.error('Xoá thất bại!');
        }
    };
    return (
        <PageContainer extra={
            <Button type="primary" onClick={() => { setEditingRecord(null); setOpen(true); form.resetFields(); }} icon={<PlusOutlined />}>
                Tạo mới
            </Button>
        }>
            <ProTable actionRef={actionRef} request={apiPlasmaList} search={{ layout: 'vertical' }}
                scroll={{ x: 'max-content' }}
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
                    { title: 'CCCD', dataIndex: 'identityNumber', search: false },
                    { title: 'SĐT', dataIndex: 'phoneNumber', align: 'center' },
                    { title: 'Email', dataIndex: 'email', search: false },
                    { title: 'Địa chỉ', dataIndex: 'address', search: false },
                    {
                        title: 'Ngày sinh', dataIndex: 'dateOfBirth', valueType: 'date', align: 'center', search: false,
                        render: (_, record) => record.dateOfBirth ? dayjs(record.dateOfBirth).format('DD-MM-YYYY') : '',
                    },
                    {
                        title: 'Ngày tạo', dataIndex: 'createdDate', valueType: 'date', search: false, align: 'center',
                        render: (_, record) => record.createdDate ? dayjs(record.createdDate).format('DD-MM-YYYY') : '',
                    },
                    {
                        title: 'Ngày cập nhật', dataIndex: 'modifiedDate', valueType: 'date', search: false, align: 'center',
                        render: (_, record) => record.modifiedDate ? dayjs(record.modifiedDate).format('DD-MM-YYYY') : ''
                    },
                    { title: 'Tổng số ngày', dataIndex: 'totalDays', align: 'center', search: false },
                    { title: 'Chuyên viên hỗ trợ', dataIndex: 'supporterName', search: false },
                    {
                        title: 'Chi nhánh', dataIndex: 'branch', search: false,
                        valueType: 'select',
                        fieldProps: {
                            options: BRANCH_OPTIONS,
                        },
                        minWidth: 100
                    },
                    {
                        title: 'Tác vụ',
                        valueType: 'option',
                        width: 60,
                        fixed: 'right',
                        render: (text, record) => [
                            <Tooltip key={`edit-${record.id}`} title="Chỉnh sửa">
                                <Button
                                    type="primary"
                                    size="small"
                                    icon={<EditOutlined />}
                                    onClick={() => {
                                        setEditingRecord(record);
                                        form.setFieldsValue({
                                            FullName: record.fullName,
                                            IdentityNumber: record.identityNumber,
                                            Gender: record.gender,
                                            Email: record.email,
                                            PhoneNumber: record.phoneNumber,
                                            TotalDays: record.totalDays,
                                            Address: record.address,
                                            SupporterId: record.supporterId,
                                            branch: record.branch,
                                            dateOfBirth: record.dateOfBirth,
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
                                        type="primary"
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
            <DrawerForm form={form} open={open} grid={true} drawerProps={{ width: 600 }}
                rowProps={{ gutter: 16 }} onOpenChange={(visible) => {
                    if (!visible) {
                        setEditingRecord(null);
                    }
                    setOpen(visible);
                }}
                onFinish={handleSubmit}
                initialValues={{
                    Name: editingRecord?.name,
                    Description: editingRecord?.description,
                    Active: editingRecord?.active,
                }}
            >
                <ProFormText
                    name="FullName"
                    label="Họ và tên"
                    rules={[{ required: true, message: 'Vui lòng nhập họ và tên' }]}
                    colProps={{ span: 12 }}
                />
                <ProFormText name="IdentityNumber" colProps={{ span: 12 }} label="CCCD"/>
                <ProFormSelect
                    rules={[{ required: true, message: 'Vui lòng chọn giới tính' }]}
                    options={[
                        { label: 'Nam', value: true as any },
                        { label: 'Nữ', value: false }
                    ]} name="Gender" label="Giới tính" colProps={{ span: 12 }} />
                <ProFormText name="Email" label="Email" colProps={{ span: 12 }}
                rules={[
                    {
                        type: 'email',
                        message: 'Email không hợp lệ',
                    }
                ]} />
                <ProFormDatePicker name="dateOfBirth" label="Ngày sinh" width="lg"
                    rules={[{ required: true, message: 'Vui lòng nhập ngày sinh' }]}
                    colProps={{ span: 12 }}
                    fieldProps={{
                        format: {
                            type: 'mask',
                            format: 'DD-MM-YYYY'
                        }
                    }} />
                <ProFormText
                    name="PhoneNumber"
                    label="SĐT"
                    rules={[
                        { required: true, message: 'Vui lòng nhập số điện thoại' },
                    ]}
                    colProps={{ span: 12 }}
                    fieldProps={{
                        inputMode: 'numeric',
                        pattern: '[0-9]*',
                        onKeyPress: (e) => {
                            if (!/[0-9]/.test(e.key)) {
                                e.preventDefault();
                            }
                        },
                    }}
                />
                <ProFormText
                    name="TotalDays"
                    label="Tổng số ngày"
                    rules={[{ required: true, message: 'Vui lòng nhập tổng số ngày' }]}
                    colProps={{ span: 12 }}
                    fieldProps={{
                        inputMode: 'numeric',
                        pattern: '[0-9]*',
                        onKeyPress: (e) => {
                            if (!/[0-9]/.test(e.key)) {
                                e.preventDefault();
                            }
                        },
                    }}
                />
                <ProFormText name="Address" label="Địa chỉ" />
                <ProFormSelect
                    name="SupporterId"
                    label="Chuyên viên hỗ trợ"
                    rules={[{ required: true, message: 'Vui lòng nhập chuyên viên hỗ trợ' }]}
                    colProps={{ span: 12 }}
                    options={techicianOptions}
                    fieldProps={{
                        onChange: (value) => {
                            const selected = techicianOptions.find((item: any) => item.value === value);
                            if (selected) {
                                console.log('Selected technician:', selected);
                                form.setFieldsValue({ branch: selected.branch });
                            }
                            else {
                                form.setFieldsValue({ branch: undefined });
                            }
                        }
                    }}
                />
                <ProFormSelect rules={[{ required: true }]} disabled
                    options={BRANCH_OPTIONS} name="branch" label="Chi nhánh" colProps={{ span: 12 }} />
            </DrawerForm>
        </PageContainer>
    );
};

export default PlasmaPage;