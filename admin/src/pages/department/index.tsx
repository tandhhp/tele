import { apiDepartmentCreate, apiDepartmentList, apiDepartmentUpdate } from "@/services/department";
import { apiBranchOptions } from "@/services/settings/branch";
import { DeleteOutlined, EditOutlined, MoreOutlined, PlusOutlined, UsergroupAddOutlined } from "@ant-design/icons";
import { ActionType, ModalForm, PageContainer, ProFormInstance, ProFormSelect, ProFormText, ProTable } from "@ant-design/pro-components"
import { history, useAccess } from "@umijs/max";
import { Button, Dropdown, message, Popconfirm } from "antd";
import { useEffect, useRef, useState } from "react";

const Index: React.FC = () => {

    const [open, setOpen] = useState<boolean>(false);
    const [department, setDepartment] = useState<any>();
    const actionRef = useRef<ActionType>();
    const formRef= useRef<ProFormInstance>();
    const access = useAccess();

    useEffect(() => {
        if (open && department) {
            formRef.current?.setFields([
                {
                    name: 'branchId',
                    value: department.branchId
                },
                {
                    name: 'name',
                    value: department.name
                },
                {
                    name: 'id',
                    value: department.id
                }
            ])
        }
    }, [open, department])

    const handleSubmit = async (values: any) => {
        if (values.id) {
            await apiDepartmentUpdate(values);
        } else {
            await apiDepartmentCreate(values);
        }
        message.success('Thành công');
        actionRef.current?.reload();
        formRef.current?.resetFields();
        setDepartment(undefined);
        return true;
    }

    return (
        <PageContainer extra={<Button type="primary" onClick={() => setOpen(true)} icon={<PlusOutlined />} hidden={!access.canAdmin}>Thêm phòng ban</Button>}>
            <ProTable
                request={apiDepartmentList}
                search={{
                    layout: 'vertical'
                }}
                columns={[
                    {
                        title: '#',
                        valueType: 'indexBorder',
                        width: 30,
                        align: 'center',
                    },
                    {
                        title: 'Tên phòng ban',
                        dataIndex: 'name',
                    },
                    {
                        title: 'Chi nhánh',
                        dataIndex: 'branchId',
                        valueType: 'select',
                        request: apiBranchOptions,
                        search: false
                    },
                    {
                        title: 'Nhóm',
                        dataIndex: 'teamCount',
                        valueType: 'digit',
                        search: false
                    },
                    {
                        title: 'Tác vụ',
                        valueType: 'option',
                        render: (_, record) => [
                            <Dropdown key="more" menu={{
                                items: [
                                    {
                                        key: 'edit',
                                        label: 'Chỉnh sửa',
                                        onClick: () => {
                                            setDepartment(record);
                                            setOpen(true);
                                        },
                                        icon: <EditOutlined />
                                    },
                                    {
                                        key: 'team',
                                        label: 'Quản lý nhóm',
                                        onClick: () => {
                                            history.push(`/user/department/team/${record.id}`);
                                        },
                                        icon: <UsergroupAddOutlined />
                                    }
                                ]
                            }}>
                                <Button type="dashed" size="small" icon={<MoreOutlined />} />
                            </Dropdown>,
                            <Popconfirm key="delete" title="Bạn có chắc muốn xóa?">
                                <Button type="primary" danger size="small" icon={<DeleteOutlined />} />
                            </Popconfirm>
                        ],
                        width: 60
                    }
                ]}
                actionRef={actionRef}
            />
            <ModalForm open={open} onOpenChange={setOpen} title="Phòng ban" onFinish={handleSubmit} formRef={formRef}>
                <ProFormText name="id" hidden />
                <ProFormSelect name="branchId" label="Chọn chi nhánh" request={apiBranchOptions} rules={[
                    {
                        required: true
                    }
                ]} allowClear={false} showSearch />
                <ProFormText name="name" label="Tên phòng ban" rules={[
                    {
                        required: true
                    }
                ]} />
            </ModalForm>
        </PageContainer>
    )
}

export default Index;